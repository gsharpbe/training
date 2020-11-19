import { ModalModule } from 'ngx-bootstrap/modal';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { DropdownModule } from 'primeng/dropdown';
import { NewLineToBreakPipe } from 'src/app/pipes/new-line-to-break';

@NgModule({
  declarations: [
    NewLineToBreakPipe
  ],
  imports: [
    // CommonModule,
    // BrowserModule,
    // FormsModule,
    // TableModule,
    // HttpClientModule
    TranslateModule,
    ModalModule.forRoot()
  ],
  exports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    TableModule,
    DropdownModule,
    HttpClientModule,
    ModalModule,
    TranslateModule,
    NewLineToBreakPipe
  ]
})
export class SharedModule { }
