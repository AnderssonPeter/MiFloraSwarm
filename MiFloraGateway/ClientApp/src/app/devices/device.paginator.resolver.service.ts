import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Device, DataLine, Span, DataPoint,  getDummyDataLine } from './device';
import { from, ReplaySubject, of, Observable } from 'rxjs';
import { delay, filter, map, first } from 'rxjs/operators';
import { GetDevicesGQL, SortOrder, DeviceSortField } from '../api/graphql/graphql.client';
import { notifyChange } from '../pagination/notifyChange';
import { Page } from '../pagination/Page';
import { Paginator } from '../pagination/Paginator';

@Injectable({
  providedIn: 'root'
})
export class DevicePaginatorResolverService implements Resolve<Paginator<Device, DeviceSortField>> {

    constructor(private readonly getDevicesGQL: GetDevicesGQL) {

    }

    async resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<Paginator<Device, DeviceSortField>> {
        //todo: try to get page, pageSize, search, orderBy, sortOrder
        const instance = new DevicePaginator(this.getDevicesGQL);
        await instance.initialize().toPromise();
        return instance;
    }
}


//Ugly solution we should be able to find a cleaner way to handle pagination its currently a big mishmash of local state and rxjs.
export class DevicePaginator implements Paginator<Device, DeviceSortField> {
    readonly contentSubject = new ReplaySubject<Page<Device>>(1);
    readonly content = this.contentSubject.asObservable();
    private initialized = false;
    private pageCount: number = 1;

    @notifyChange
    page: number = 1;

    @notifyChange
    pageSize: number = 10;

    @notifyChange
    search: string = "";

    @notifyChange
    orderBy: DeviceSortField = DeviceSortField.Name;

    @notifyChange
    sortOrder: SortOrder = SortOrder.Ascending;

    isLoading: boolean = false;
    activeQueries = 0;
    versionCounter: number = 0;
    currentVersion: number = 0;

    constructor(private readonly getDevicesGQL: GetDevicesGQL) { }

    initialize() {
        this.initialized = true;
        return this.update();
    }

    setPage(page: number) {
        if (page <= 0 || page > this.pageCount)
            return;
        this.page = page;
    }

    sortBy(sortField: DeviceSortField)
    {
        if (this.orderBy == sortField) {
            this.sortOrder = this.sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
        else {
            this.sortOrder = SortOrder.Ascending;
            this.orderBy = sortField;
        }
    }

    update() {
        if (!this.initialized) {
            return of<void>();
        }
        this.activeQueries++;
        this.isLoading = true;
        const version = ++this.versionCounter;
        const observable = this.getDevicesGQL.fetch({ page: this.page, pageSize: this.pageSize, search: this.search, orderBy: this.orderBy, sortOrder: this.sortOrder });
        const filterLoading = observable.pipe(filter(query => !query.loading)).pipe(first());
        filterLoading.subscribe((query) => {
            if (version <= this.currentVersion) {
                return;
            }
            this.currentVersion = version;
            if (query.errors != undefined) {
                this.contentSubject.error(query.errors);
            }
            else if (query.data && query.data.devicePager && query.data.devicePager.devices) {
                this.pageCount = query.data.devicePager.pageCount;
                this.contentSubject.next({
                    pageCount: query.data.devicePager.pageCount,
                    items: query.data.devicePager.devices?.map(device => {
                        const dataLine = new DataLine('', '', device.failuresLast24Hours!.map(failure => ({ when: failure.when, value: 1 })));
                        return new Device(
                            device?.name ?? 'N/A',
                            dataLine,
                            device?.macAddress ?? 'N/A',
                            device?.ipAddress ?? 'N/A',
                            'N/A',
                            'N/A',
                            device.tags!.map(x => x.tag + ': ' + x.value)
                        );
                    })
                });
            }
        }, (error) => {
            this.contentSubject.error(error);
        },
        () => {
            this.activeQueries--;
            this.isLoading = this.activeQueries > 0;
        });
        return filterLoading.pipe(map(x => {})).pipe(first());
    }
}