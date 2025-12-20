import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AuditLogComponent } from './audit-log.component';
import { AuditService } from '../../services';

describe('AuditLogComponent', () => {
  let component: AuditLogComponent;
  let fixture: ComponentFixture<AuditLogComponent>;

  const mockAuditService = {
    getLogs: () => of([]),
    requestExport: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuditLogComponent],
      providers: [
        { provide: AuditService, useValue: mockAuditService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AuditLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
