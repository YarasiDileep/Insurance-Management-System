import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersService, CustomerDto } from '../../core/customers.service';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h2>Dashboard</h2>
      <section>
        <h3>Customers</h3>
        <ul>
          <li *ngFor="let c of customers">{{ c.name }} ({{ c.email }})</li>
        </ul>
      </section>
    </div>
  `
})
export class DashboardComponent {
  customers: CustomerDto[] = [];

  constructor(private customersService: CustomersService) {
    this.customersService.list().subscribe({ next: data => (this.customers = data) });
  }
}
