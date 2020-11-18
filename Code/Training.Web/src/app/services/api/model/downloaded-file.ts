import * as FileSaver from 'file-saver';

export class DownloadedFile {
    constructor(public blob: Blob, public fileName: string) { }

    save(status: string = '') {
        if (status !== '') {
            this.fileName = this.setFileName(status);
        }
        FileSaver.saveAs(this.blob, this.fileName);
    }

    setFileName(status: string) {
        const ext = '.' + this.fileName.split('.').pop();
        const name = this.fileName.replace(ext, '');
        const filename = name + status + ext;
        return filename;
    }
}
