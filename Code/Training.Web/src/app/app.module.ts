import { CustomerApiService } from './services/api/customer-api.service';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { MenuComponent } from './modules/app/components/menu/menu.component';
import { AppComponent } from './modules/app/components/app/app.component';
import { SharedModule } from './modules/shared/shared.module';
import { ProjectsModule } from './modules/projects/projects.module';
import { CustomersModule } from './modules/customers/customers.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent
  ],
  imports: [
    SharedModule,
    AppRoutingModule,
    CustomersModule,
    ProjectsModule,
    BrowserAnimationsModule,
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
      }
  })
  ],
  providers: [
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
