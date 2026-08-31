import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface PaymentDto { id: number; paymentRef: string; amount: number; status: string }

@Component({
  standalone: true,
  selector: 'app-payments',
  imports: [CommonModule],
  template: `
    <div class="payments">
      <h2>Payments</h2>
      <ul>
        <li *ngFor="let p of payments">{{ p.paymentRef }} - {{ p.amount | currency }} ({{ p.status }})</li>
      </ul>
    </div>
  `
})
export class PaymentsComponent {
  payments: PaymentDto[] = [];
  constructor(private http: HttpClient) { this.http.get<PaymentDto[]>('/api/Payments').subscribe({ next: p => this.payments = p }); }
}
