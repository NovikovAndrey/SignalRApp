export class BlobModel{
  blob: string;
  type: string;
  constructor (_blob?: string, _type?: string)
  {
    this.blob = _blob;
    this.type = _type;
  }
}
