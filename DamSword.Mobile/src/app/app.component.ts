import { Component, ViewChild } from '@angular/core';
import { Nav, Platform } from 'ionic-angular';
import { StatusBar, Splashscreen } from 'ionic-native';

import { Dashboard } from '../pages/dashboard/details';


@Component({
    templateUrl: 'app.html'
})
export class MyApp {
    @ViewChild(Nav)
    nav: Nav;

    rootPage: any = Dashboard;

    constructor(public platform: Platform) {
        this.initializeApp();
    }

    initializeApp() {
        this.platform.ready().then(() => {
            StatusBar.styleBlackOpaque();
            Splashscreen.hide();
        });
    }

    getPages(): Array<{ title: string, component: any }> {
        return [
            { title: 'Dashboard', component: Dashboard }
        ];
    }

    openPage(page) {
        this.nav.setRoot(page.component);
    }
}