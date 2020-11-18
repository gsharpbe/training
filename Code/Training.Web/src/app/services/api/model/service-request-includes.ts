export class ServiceRequestIncludes {

    private includes = new Array();

    public includeAll = false;

    public add(model: string, property: string): this {
        this.includes.push({ model: model, property: property });

        return this;
    }

    public getHeaders() {
        const headers = new Array();

        if (this.includeAll) {
            headers.push({ key: 'include-all', value: 'true' });
            return headers;
        }

        headers.push({ key: 'include-all', value: 'false' });

        if (this.includes.length > 0) {
            const models = this.includes.map(include => include.model);
            const distinctModels = models.filter((value, index) => models.indexOf(value) === index);

            distinctModels.forEach(model => {
                const properties = this.includes.filter(include => include.model === model).map(include => include.property).join();
                headers.push({ key: `include-${model}`, value: properties });
            });
        }

        return headers;
    }
}
