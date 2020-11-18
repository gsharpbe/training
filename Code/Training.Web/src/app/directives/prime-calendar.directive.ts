import { Directive, Host, Optional, Self } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Calendar } from 'primeng/calendar';

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: 'p-calendar'
})
export class PrimeCalendarDirective {
    constructor(@Host() @Self() @Optional() public host: Calendar,
    public translateService: TranslateService) {
        host.showWeek = true;
        host.locale.firstDayOfWeek = 1;
        if (translateService.currentLang === 'nl') {
            host.locale.dayNames = ['Zondag', 'Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag'];
            host.locale.dayNamesMin = ['Zo', 'Ma', 'Di', 'Wo ', 'Do', 'Vr ', 'Za'];
            host.locale.dayNamesShort = ['Zon', 'Maa', 'Din', 'Woe', 'Don', 'Vrij', 'Zat'];
            host.locale.monthNames = ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni', 'Juli', 'Augustus', 'September', 'Oktober', 'November', 'December' ];
            host.locale.monthNamesShort = ['jan', 'feb', 'mrt', 'apr', 'mei', 'jun', 'jul', 'aug', 'sep', 'okt', 'nov', 'dec' ];
            host.locale.weekHeader = 'Week';
            host.locale.today = 'Vandaag';
        }
        if (translateService.currentLang === 'fr') {
            host.locale.dayNames = ['Dimanche', 'Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi'];
            host.locale.dayNamesMin = ['Di', 'Lu', 'Ma', 'Me', 'Je', 'Ve', 'Sa'];
            host.locale.dayNamesShort = ['Dim', 'Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam'];
            host.locale.monthNames = ['Janvier', 'Février', 'Mars', 'Avril', 'Mai', 'Juin', 'Juillet', 'Août', 'Septembre', 'Octobre', 'Novembre', 'Décembre' ];
            host.locale.monthNamesShort = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jul', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc' ];
            host.locale.weekHeader = 'Semaine';
            host.locale.today = 'Aujourd\'hui';
        }
    }
}
