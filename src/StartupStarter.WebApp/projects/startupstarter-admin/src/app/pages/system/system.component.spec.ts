import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { SystemComponent } from './system.component';
import { SystemService } from '../../services';

describe('SystemComponent', () => {
  let component: SystemComponent;
  let fixture: ComponentFixture<SystemComponent>;

  const mockSystemService = {
    getHealth: () => of({}),
    getMetrics: () => of({}),
    getMaintenances: () => of([]),
    getBackups: () => of([]),
    getErrors: () => of([]),
    triggerBackup: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SystemComponent],
      providers: [
        { provide: SystemService, useValue: mockSystemService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
