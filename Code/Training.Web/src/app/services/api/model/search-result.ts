export class SearchResult<T> {
    total?: number;
    startIndex: number;
    itemsCount: number;
    data: T[];
}
