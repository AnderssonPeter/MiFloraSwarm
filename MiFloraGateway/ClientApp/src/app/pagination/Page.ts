
export interface Page<TItem> {
    readonly items: ReadonlyArray<TItem>;
    readonly pageCount: number;
}
