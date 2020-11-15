namespace Metanous.Model.Core.Filter
{
    //The json property names map on the query parameters sent by the Kendo UI grid component. 
    //This has the advantage that this component can be plugged into the service very quickly. 
    //Please do not change do not change these ! 
    public enum FilterOperator
    {
        Eq, //equal to
        Neq, //not equal to
        IsNull, //is equal to null
        IsNotNull, //is not equal to null
        Lt, //less than
        Lte, //less than or equal to
        Gt, //greater than
        Gte, //greater than or equal to
        StartsWith, //only for string fields
        EndsWith, //only for string fields
        Contains, //only for string fields
        DoesNotContain, //only for string fields
        IsEmpty, //only for string fields
        IsNotEmpty //only for string fields
    }
}