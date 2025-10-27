import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { CardComponent } from './card.component';

describe('CardComponent', () => {
  let component: CardComponent;
  let fixture: ComponentFixture<CardComponent>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CardComponent],
      providers: [{ provide: Router, useValue: routerSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(CardComponent);
    component = fixture.componentInstance;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default values', () => {
    expect(component.currentUser).toBeNull();
    expect(component.selectedCard).toBeNull();
  });

  it('should set current user on init', () => {
    component.ngOnInit();
    expect(component.currentUser).toBeTruthy();
    expect(component.currentUser.name).toBe('Utilisateur Test');
  });

  it('should select card and process payment', () => {
    spyOn(console, 'log');
    component.selectCard('success');

    expect(component.selectedCard).toBe('success');
    expect(console.log).toHaveBeenCalledWith('Carte sélectionnée:', 'success');
    expect(console.log).toHaveBeenCalledWith('Paiement réussi simulé');
  });

  it('should handle insufficient funds scenario', () => {
    spyOn(console, 'log');
    component.selectCard('insufficient');

    expect(component.selectedCard).toBe('insufficient');
    expect(console.log).toHaveBeenCalledWith('Fond insuffisant simulé');
  });

  it('should handle declined card scenario', () => {
    spyOn(console, 'log');
    component.selectCard('declined');

    expect(component.selectedCard).toBe('declined');
    expect(console.log).toHaveBeenCalledWith('Carte rejetée simulée');
  });

  it('should handle fraudulent card scenario', () => {
    spyOn(console, 'log');
    component.selectCard('fraudulent');

    expect(component.selectedCard).toBe('fraudulent');
    expect(console.log).toHaveBeenCalledWith('Carte frauduleuse simulée');
  });

  it('should logout and navigate to login', () => {
    spyOn(console, 'log');
    component.logout();

    expect(console.log).toHaveBeenCalledWith("Déconnexion de l'utilisateur");
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
  });
});
