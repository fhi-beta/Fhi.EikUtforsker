import { WebDavResource } from "./WebDavResource";

export class FolderEntry {
  public isOpen: boolean;
  public isFile: boolean;
  public hasInsertedChildren: boolean = false;

  private _children: FolderEntry[] = [];
  get children(): FolderEntry[] { return this._children; };
  set children(value: FolderEntry[]) { this._children = value; this.hasInsertedChildren = true; }

  constructor(public resource: WebDavResource) {
    this.isFile = !resource.isCollection;
    this.isOpen = false;
  }
}
