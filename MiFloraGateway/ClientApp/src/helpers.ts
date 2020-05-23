export function distinct<T>(value: T, index: number, arr: T[]) {
    return arr.indexOf(value) == index;
}