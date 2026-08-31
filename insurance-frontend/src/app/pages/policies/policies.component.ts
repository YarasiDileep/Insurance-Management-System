import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface PolicyDto { id: number; policyNumber: string; holderName: string; status: string }

@Component({
  standalone: true,
  selector: 'app-policies',
  imports: [CommonModule],
  template: `
    <div class="policies">
      <h2>Policies</h2>
      <ul>
        <li *ngFor="let p of policies">{{ p.policyNumber }} - {{ p.holderName }} ({{ p.status }})</li>
      </ul>
    </div>
  `
})
export class PoliciesComponent {
  policies: PolicyDto[] = [];
  constructor(private http: HttpClient) { this.http.get<PolicyDto[]>('/api/Policies').subscribe({ next: p => this.policies = p }); }
}
