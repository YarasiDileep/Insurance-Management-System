import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, FormGroup } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="login">
      <h1>Sign in</h1>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <input formControlName="username" placeholder="Username" />
        <input formControlName="password" type="password" placeholder="Password" />
        <button type="submit">Sign in</button>
      </form>
    </div>
  `,
  styles: [`.login { max-width: 360px; margin: 2rem auto; } input { display:block; width:100%; margin-bottom:.5rem; padding:.5rem }`]
})
export class LoginComponent {
  form!: FormGroup;

  constructor(private formBuilder: FormBuilder, private auth: AuthService) {
    // Set sensible defaults to make local testing easier (password matches seeded users)
    this.form = this.formBuilder.group({ username: ['admin'], password: ['Password123!'] });
  }

  submit() {
    if (!this.form.valid) return;
    const { username, password } = this.form.value as { username: string; password: string };
    this.auth.login(username, password).subscribe({ next: () => window.location.reload() });
  }
}
