import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { WebhookListComponent } from './webhook-list.component';
import { WebhookService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';

describe('WebhookListComponent', () => {
  let component: WebhookListComponent;
  let fixture: ComponentFixture<WebhookListComponent>;

  const mockWebhookService = {
    getAll: () => of([]),
    getById: () => of({}),
    create: () => of({}),
    update: () => of({}),
    delete: () => of({}),
    test: () => of({}),
    enable: () => of({}),
    disable: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WebhookListComponent],
      providers: [
        provideRouter([]),
        { provide: WebhookService, useValue: mockWebhookService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(WebhookListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
