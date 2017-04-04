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

    pages: Array<{ title: string, component: any }>;

    constructor(public platform: Platform) {
        this.initializeApp();

        this.pages = [
            { title: 'Dashboard', component: Dashboard }
        ];

    }

    initializeApp() {
        this.platform.ready().then(() => {
            StatusBar.styleBlackOpaque();
            Splashscreen.hide();
        });
    }

    openPage(page) {
        this.nav.setRoot(page.component);
    }
}