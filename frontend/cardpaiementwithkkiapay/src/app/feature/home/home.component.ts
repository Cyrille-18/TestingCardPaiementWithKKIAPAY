import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth/services/auth.service';

interface User {
  name: string;
  email: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  currentUser: User | null = null;

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
  }

  selectCard(): void {}

  logout(): void {
    this.authService.logout();
  }
}
