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
  ],
  providers: [
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
