import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit } from '@angular/core';
import { SelectItem } from 'primeng/api';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  languages: SelectItem[];
  selectedLanguage: string;

  constructor(private translateService: TranslateService) { }

  ngOnInit(): void {
    this.initializeLanguages();
  }

  initializeLanguages(): void {
    this.languages = [{ label: 'English', value: 'en' }, { label: 'Nederlands', value: 'nl' }];
  }

  onLanguageChanged(): void {
    this.translateService.use(this.selectedLanguage);
  }

}
