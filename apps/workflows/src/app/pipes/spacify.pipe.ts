import {
    Pipe,
    PipeTransform
} from '@angular/core';

import { Strings } from '../models';

@Pipe({
    name: 'spacify'
})
export class SpacifyPipe implements PipeTransform {
    transform(value: string) {
        return Strings.spacify(value);
    }
}
