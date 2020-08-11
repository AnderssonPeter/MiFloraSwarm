import { Injectable } from '@angular/core';
import { OnboardingClient, SetupModel } from '../api/rest/rest.client';
import { ReplaySubject } from 'rxjs';
@Injectable({
    providedIn: 'root'
})
export default class OnboardingService {
    private readonly isSetupSubject = new ReplaySubject<boolean>(1);
    public readonly isSetup = this.isSetupSubject.asObservable();
    constructor(private readonly onboardingClient: OnboardingClient) {
        onboardingClient.isSetup()
            .toPromise()
            .then(result => this.isSetupSubject.next(result))
            .catch((error) => {
                console.log('Failed to check if system is setup!', error);
            });
    }
    async setupAsync(model: SetupModel) {
        const result = await this.onboardingClient.setup(model).toPromise();
        this.isSetupSubject.next(true);
        return result;
    }
}
