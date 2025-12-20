import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { WebhookFormComponent } from './webhook-form.component';
import { WebhookService } from '../../../services';

describe('WebhookFormComponent', () => {
  let component: WebhookFormComponent;
  let fixture: ComponentFixture<WebhookFormComponent>;

  const mockWebhookService = {
    getAll: () => of([]),
    getById: () => of({}),
    create: () => of({}),
    update: () => of({}),
    delete: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WebhookFormComponent],
      providers: [
        provideRouter([]),
        { provide: WebhookService, useValue: mockWebhookService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(WebhookFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
