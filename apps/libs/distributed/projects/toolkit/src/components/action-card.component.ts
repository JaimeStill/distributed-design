import {
    AfterContentChecked,
    Component,
    Input
} from '@angular/core';

import {
    FxCross,
    FxDirection,
    FxMain
} from '../types/flex';

export type ActionCardOrientation = 'top' | 'right' | 'bottom' | 'left';

@Component({
    selector: 'action-card',
    templateUrl: 'action-card.component.html'
})
export class ActionCardComponent implements AfterContentChecked {
    layout: FxDirection = 'column';
    actionLayout: FxDirection = 'row';
    computedStyle: string = 'rounded-boto';

    @Input() orientation: ActionCardOrientation = 'top';
    @Input() main: FxMain = 'space-between';
    @Input() cross: FxCross = 'center';
    @Input() size: number | string = 'auto';
    @Input() cardStyle = 'm4 rounded border-divider';
    @Input() actionStyle = 'background-default';
    @Input() roundActions = true;
    @Input() actionPadding: number | string = 'inherit';

    ngAfterContentChecked(): void {
        switch (this.orientation) {
            case 'top':
                this.layout = 'column';
                this.actionLayout = 'row';
                this.computedStyle = this.roundActions
                    ? 'rounded-top ' + this.actionStyle.trim()
                    : this.actionStyle;
                break;
            case 'right':
                this.layout = 'row-reverse';
                this.actionLayout = 'column';
                this.computedStyle = this.roundActions
                    ? 'rounded-right ' + this.actionStyle.trim()
                    : this.actionStyle;
                break;
            case 'bottom':
                this.layout = 'column-reverse';
                this.actionLayout = 'row';
                this.computedStyle = this.roundActions
                    ? 'rounded-bottom ' + this.actionStyle.trim()
                    : this.actionStyle;
                break;
            case 'left':
                this.layout = 'row';
                this.actionLayout = 'column';
                this.computedStyle = this.roundActions
                    ? 'rounded-left ' + this.actionStyle.trim()
                    : this.actionStyle;
                break;
        }
    }
}