import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from "./shared/components/navbar/navbar.component";
import { HttpClientModule } from '@angular/common/http';
import { httpInterceptorProviders } from './core/services/auth.interceptor';
import { AuthGuard } from './core/services/auth.guard';
import { NgToastModule } from 'ng-angular-popup';

@NgModule({
    declarations: [
        AppComponent,
    ],
    providers: [
        httpInterceptorProviders,
        AuthGuard
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        NavbarComponent,
        HttpClientModule,
        NgToastModule
    ]
})
export class AppModule { }
