import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PageHeaderComponent } from './page-header.component';

describe('PageHeaderComponent', () => {
  let component: PageHeaderComponent;
  let fixture: ComponentFixture<PageHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PageHeaderComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(PageHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have default empty title', () => {
    expect(component.title).toBe('');
  });

  it('should have default empty subtitle', () => {
    expect(component.subtitle).toBe('');
  });

  it('should have default empty icon', () => {
    expect(component.icon).toBe('');
  });

  it('should set title input', () => {
    const testTitle = 'Test Page Header';
    component.title = testTitle;
    fixture.detectChanges();
    expect(component.title).toBe(testTitle);
  });

  it('should set subtitle input', () => {
    const testSubtitle = 'Test Subtitle';
    component.subtitle = testSubtitle;
    fixture.detectChanges();
    expect(component.subtitle).toBe(testSubtitle);
  });

  it('should set icon input', () => {
    const testIcon = 'dashboard';
    component.icon = testIcon;
    fixture.detectChanges();
    expect(component.icon).toBe(testIcon);
  });
});
