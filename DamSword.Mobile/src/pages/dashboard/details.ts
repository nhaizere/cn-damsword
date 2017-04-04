import { Component } from '@angular/core';

import { NavController } from 'ionic-angular';

@Component({
    selector: 'dashboard-details',
    templateUrl: 'details.html'
})
export class Dashboard {

    constructor(public navCtrl: NavController) {
    }

    onLink(url: string) {
        window.open(url);
    }
}
