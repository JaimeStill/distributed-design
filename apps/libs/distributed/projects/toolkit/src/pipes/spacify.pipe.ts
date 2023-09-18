import {
    Pipe,
    PipeTransform
} from '@angular/core';

import { Strings } from '@distributed/core';

@Pipe({
    name: 'spacify'
})
export class SpacifyPipe implements PipeTransform {
    transform(value: string) {
        return Strings.spacify(value);
    }
}
