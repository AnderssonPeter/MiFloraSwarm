import { Directive, ElementRef, Input, OnChanges, SimpleChanges, OnInit, OnDestroy } from '@angular/core';

//Ugly solution to show a loading indicator, in the long run we should create something thats not a hack!
@Directive({
    selector: '[appLoading]'
})
export class LoadingDirective implements OnChanges, OnInit, OnDestroy {
    private readonly element: HTMLElement
    private readonly parentElement?: HTMLElement;
    private loadingElement: HTMLDivElement | null = null;
    @Input('appLoading') loading: boolean = false;
    @Input('loadingText') loadingText?: string;

    private get isActive() {
        return this.loadingElement?.parentElement != null;
    }

    constructor(el: ElementRef) {
        if (!(el.nativeElement instanceof HTMLElement)) {
            throw new Error("Element must be a html element");
        }
        this.element = el.nativeElement as HTMLElement;

        if (this.element instanceof HTMLTableSectionElement) {
            if (this.element.parentElement instanceof HTMLTableElement) {
                this.parentElement = this.element.parentElement;
            }
        }
    }

    ngOnInit(): void {
        this.createLoadingIndicator();
    }

    ngOnDestroy(): void {
        this.destroyLoadingIndicator();
    }

    ngOnChanges(_: SimpleChanges): void {
        this.updateLoadingIndicator();
        if (this.loading && !this.isActive) {
            this.showLoadingIndicator();
        }
        else if (!this.loading && this.isActive) {
            this.hideLoadingIndicator();
        }
    }

    updateLoadingIndicator() {
        if (this.loadingElement) {
            const containerElement = this.loadingElement.childNodes.item(0) as HTMLDivElement;
            const textElement = containerElement.childNodes.item(1) as HTMLDivElement;
            textElement.innerText = this.loadingText ?? '';

            if (this.parentElement) {
                this.parentElement.style.display = 'relative';
                this.loadingElement.style.top = this.parentElement.offsetTop + this.element.offsetTop + 'px';
                this.loadingElement.style.left = this.parentElement.offsetLeft + this.element.offsetLeft + 'px';
                this.loadingElement.style.height = this.element.clientHeight + 'px';
                this.loadingElement.style.width = this.element.clientWidth + 'px';
            }
            else {
                this.element.style.display = 'relative';
                this.loadingElement.style.top = this.element.offsetTop + 'px';
                this.loadingElement.style.left = this.element.offsetLeft + 'px';
                this.loadingElement.style.height = this.element.clientHeight + 'px';
                this.loadingElement.style.width = this.element.clientWidth + 'px';
            }
        }
    }

    showLoadingIndicator() {
        if (this.loadingElement) {
            if (this.parentElement) {
                this.parentElement.parentElement?.appendChild(this.loadingElement);
            }
            else {
                this.element.parentElement?.appendChild(this.loadingElement);
            }
        }
    }

    hideLoadingIndicator() {
        if (this.loadingElement) {
            if (this.parentElement) {
                this.parentElement.parentElement?.removeChild(this.loadingElement);
            }
            else {
                this.element.parentElement?.removeChild(this.loadingElement);
            }
        }
    }

    createLoadingIndicator() {
        const loadingElement = document.createElement('div');
        loadingElement.style.background = 'rgba(0, 0, 0, 0.1)';
        loadingElement.style.display = 'relative';
        loadingElement.style.overflow = 'hidden';

        const container = document.createElement('div');
        loadingElement.appendChild(container);
        container.style.position = 'absolute';
        container.style.margin = '0';
        container.style.top = '50%';
        container.style.left = '50%';
        container.style.transform = 'translate(-50%, -50%)';
        const loadingImage = document.createElement('img');
        loadingImage.src = 'assets/loading_black.png';
        container.appendChild(loadingImage);

        container.style.textAlign = 'center';
        const loadingText = document.createElement('div');
        loadingText.style.fontWeight = 'bold';
        container.appendChild(loadingText);

        this.loadingElement = loadingElement;
        loadingElement.style.position = 'absolute';
    }

    private destroyLoadingIndicator() {
        if (this.isActive) {
            this.hideLoadingIndicator();
        }
        this.loadingElement = null;
    }
}
