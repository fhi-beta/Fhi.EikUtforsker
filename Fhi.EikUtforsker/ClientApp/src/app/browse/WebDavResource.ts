export interface WebDavResource {
  uri: string;
  name: string;
  eTag: string;
  lastModifiedDate: Date;
  contentLength: number;
  isCollection: boolean;
}
