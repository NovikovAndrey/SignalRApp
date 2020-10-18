import { NgModule } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';

@NgModule({
  imports: [
    MatTableModule,
    MatSelectModule
  ],
  exports: [
    MatTableModule,
    MatSelectModule
  ]
})

export class AngularMaterialModule { }