import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, inject, runInInjectionContext } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Observable } from 'rxjs';
import { Coupon } from '../models/coupon.model';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HttpClientModule, AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  http = inject(HttpClient)
  
  coupons$ = this.getCoupons()

  private getCoupons() : Observable<Coupon[]> {
    return this.http.get<Coupon[]>('https://localhost:7001/api/coupons')
  }
}
