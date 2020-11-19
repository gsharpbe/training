import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BrowserModule,
    FormsModule
  ],
  exports: [
    CommonModule,
    BrowserModule,
    FormsModule
  ]
})
export class SharedModule { }
