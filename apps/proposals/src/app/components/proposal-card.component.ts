import {
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output
} from '@angular/core';

import {
    Observable,
    Subscription
} from 'rxjs';

import {
    SnackerService,
    WorkflowsGateway
} from '@distributed/toolkit';

import {
    Status,
    Statuses
} from '@distributed/contracts';

import {
    Package,
    PackageStates
} from '@workflows/contracts';

import { TooltipPosition } from '@angular/material/tooltip';
import { EventMessage } from '@distributed/core';
import { Proposal } from '../models';
import { ProposalQuery } from '../services';

@Component({
    selector: 'proposal-card',
    templateUrl: 'proposal-card.component.html',
    providers: [
        ProposalQuery,
        WorkflowsGateway
    ]
})
export class ProposalCardComponent implements OnInit, OnDestroy {
    private sub: Subscription;

    package: Package | null = null;
    packages: Package[] | null = null;
    status: Status | null = null;

    @Input() proposal: Proposal;
    @Input() size: number | string = 360;
    @Input() cardStyle: string = 'm4 rounded border-divider background-card';
    @Input() tooltipLocation: TooltipPosition = 'below';
    @Input() trigger: Observable<EventMessage<Package>>;

    @Input() showActions: boolean = true;
    @Input() showDetails: boolean = true;

    @Output() edit = new EventEmitter<Proposal>();
    @Output() submit = new EventEmitter<Package>();
    @Output() withdraw = new EventEmitter<Package>();
    @Output() remove = new EventEmitter<Proposal>();

    constructor(
        private proposalQuery: ProposalQuery,
        private snacker: SnackerService,
        private workflowGwy: WorkflowsGateway
    ) { }

    private loadStatus = async () =>
        this.status = await this.proposalQuery.getStatus(this.proposal?.id);

    private loadPackage = async () =>
        this.package = await this.workflowGwy.getActivePackage(
            this.proposal?.id,
            this.proposal?.type
        );

    private loadPackages = async () =>
        this.packages = await this.workflowGwy.getPackagesByEntity(
            this.proposal?.id,
            this.proposal?.type
        );

    private async load() {
        await this.loadStatus();
        await this.loadPackage();
        await this.loadPackages();
    }

    private async handlePackageEvent(event: EventMessage<Package>) {
        if (
            event.data
            && event.data.entityId === this.proposal.id
            && event.data.entityType === this.proposal.type
        ) {
            this.snacker.sendSuccessMessage(event.message);
            await this.loadPackage();
            await this.loadPackages();
        }
    }

    async ngOnInit(): Promise<void> {
        if (this.trigger) {
            this.sub = this.trigger.subscribe((event: EventMessage<Package>) =>
                this.handlePackageEvent(event)
            );
        }

        await this.load();
    }

    ngOnDestroy(): void {
        this.sub?.unsubscribe();
    }

    canSubmit = (): boolean =>
        this.package === null
        || this.package.state === PackageStates.Returned;

    canWithdraw = (): boolean =>
        !this.canSubmit()
        && this.package !== null;

    submitPackage() {
        const pkg = this.package?.id > 0
            ? this.package
            : <Package>{
                state: PackageStates.Pending,
                result: Statuses.Active,
                entityId: this.proposal.id,
                entityType: this.proposal.type
            }

        this.submit.emit(pkg);
    }
}