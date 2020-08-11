import { Observable } from 'rxjs';
import { SortOrder } from '../api/graphql/graphql.client';
import { Page } from './Page';

export interface Paginator<TItem, TSortFieldEnum> {
    page: number;
    pageSize: number;
    search: string;
    orderBy: TSortFieldEnum;
    sortOrder: SortOrder;


    readonly isLoading: boolean;
    readonly content: Observable<Page<TItem>>;

    initialize(): Observable<void>;
    update(): Observable<void>;

    setPage(page: number): void;
    sortBy(sortField: TSortFieldEnum): void;
}
