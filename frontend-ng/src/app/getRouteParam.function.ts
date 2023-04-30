import { ActivatedRouteSnapshot } from '@angular/router';

export function getRouteData(snapshot: ActivatedRouteSnapshot, param: string) {
    let p = snapshot.data[param];
    if(!p && snapshot.parent) {
        p = getRouteData(snapshot.parent, param);
    }
    return p;
}