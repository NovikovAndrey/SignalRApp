export class ConfigModel{
  chanelUrl: string='';
  methodName:string='';
  constructor (url: string, method: string){
    this.chanelUrl = url;
    this.methodName = method;
  }
}
