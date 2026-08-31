import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface ClaimDto { id: number; claimNumber: string; policyNumber: string; status: string }

@Component({
  standalone: true,
  selector: 'app-claims',
  imports: [CommonModule],
  template: `
    <div class="claims">
      <h2>Claims</h2>
      <ul>
        <li *ngFor="let c of claims">{{ c.claimNumber }} - {{ c.policyNumber }} ({{ c.status }})</li>
      </ul>
    </div>
  `
})
export class ClaimsComponent {
  claims: ClaimDto[] = [];
  constructor(private http: HttpClient) { this.http.get<ClaimDto[]>('/api/Claims').subscribe({ next: c => this.claims = c }); }
}
