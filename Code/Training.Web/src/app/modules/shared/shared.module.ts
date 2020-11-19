import { ModalModule } from 'ngx-bootstrap/modal';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [],
  imports: [
    // CommonModule,
    // BrowserModule,
    // FormsModule,
    // TableModule,
    // HttpClientModule
    ModalModule.forRoot()
  ],
  exports: [
    CommonModule,
    BrowserModule,
    FormsModule,
    TableModule,
    HttpClientModule,
    ModalModule
  ]
})
export class SharedModule { }
