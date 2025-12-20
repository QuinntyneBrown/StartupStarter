import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ContentFormComponent } from './content-form.component';
import { ContentService } from '../../../services';

describe('ContentFormComponent', () => {
  let component: ContentFormComponent;
  let fixture: ComponentFixture<ContentFormComponent>;

  const mockContentService = {
    getAll: () => of([]),
    getById: () => of({}),
    create: () => of({}),
    update: () => of({}),
    delete: () => of({})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContentFormComponent],
      providers: [
        provideRouter([]),
        { provide: ContentService, useValue: mockContentService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ContentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
