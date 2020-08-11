export function isMinimumLength(x: any): x is { minimumLength: number; } {
    return typeof (x) === 'object' && x != null && 'minimumLength' in x && typeof (x.minimumLength) === 'number';
}
