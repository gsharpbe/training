export abstract class ModelBase {
    guid: string;
    isObsolete: boolean;

    constructor() {
        this.isObsolete = false;
    }
}
