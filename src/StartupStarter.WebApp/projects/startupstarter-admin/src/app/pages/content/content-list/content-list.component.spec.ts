import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ContentListComponent } from './content-list.component';
import { ContentService } from '../../../services';
import { MatDialog } from '@angular/material/dialog';

describe('ContentListComponent', () => {
  let component: ContentListComponent;
  let fixture: ComponentFixture<ContentListComponent>;

  const mockContentService = {
    getAll: () => of([]),
    getById: () => of({}),
    create: () => of({}),
    update: () => of({}),
    delete: () => of({}),
    publish: () => of({}),
    unpublish: () => of({})
  };

  const mockDialog = {
    open: jasmine.createSpy('open').and.returnValue({
      afterClosed: () => of(false)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContentListComponent],
      providers: [
        provideRouter([]),
        { provide: ContentService, useValue: mockContentService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ContentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
