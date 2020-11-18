export class FilterOperator {
    static equalTo = 'Eq'; // equal to
    static notEqualTo = 'Neq'; // not equal to
    static isNull = 'IsNull'; // is equal to null
    static isNotNull = 'IsNotNull'; // is not equal to null
    static lessThan = 'Lt'; // less than
    static lessThanOrEqualTo = 'Lte'; // less than or equal to
    static greaterThan = 'Gt'; // greater than
    static greaterThanOrEqualTo = 'Gte'; // greater than or equal to
    static startsWith = 'StartsWith'; // only for string fields
    static endsWith = 'EndsWith'; // only for string fields
    static contains = 'Contains'; // only for string fields
    static doesNotContain = 'DoesNotContain'; // only for string fields
    static isEmpty = 'IsEmpty'; // only for string fields
    static isNotEmpty = 'IsNotEmpty'; // only for string fields

    // primeNG, "startsWith", "contains", "endsWidth", "equals", "notEquals" and "in".
    static equals = FilterOperator.equalTo;
    static notEquals = FilterOperator.notEqualTo;
    static in = FilterOperator.contains;

    [s: string]: string;
    equalTo = FilterOperator.equalTo;
    notEqualTo = FilterOperator.notEqualTo;
    isNull = FilterOperator.isNull;
    isNotNull = FilterOperator.isNotNull;
    lessThan = FilterOperator.lessThan;
    lessThanOrEqualTo = FilterOperator.lessThanOrEqualTo;
    greaterThan = FilterOperator.greaterThan;
    greaterThanOrEqualTo = FilterOperator.greaterThanOrEqualTo;
    startsWith = FilterOperator.startsWith;
    endsWith = FilterOperator.endsWith;
    contains = FilterOperator.contains;
    doesNotContain = FilterOperator.doesNotContain;
    isEmpty = FilterOperator.isEmpty;
    isNotEmpty = FilterOperator.isNotEmpty;
    equals = FilterOperator.equals;
    notEquals = FilterOperator.notEquals;
    in = FilterOperator.in;
}
