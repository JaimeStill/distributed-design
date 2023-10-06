import {
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output
} from '@angular/core';

import {
    PackageListener,
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
        PackageListener,
        ProposalQuery,
        WorkflowsGateway
    ]
})
export class ProposalCardComponent implements OnInit {
    package: Package | null = null;
    packages: Package[] | null = null;
    status: Status | null = null;

    @Input() proposal: Proposal;
    @Input() size: number | string = 360;
    @Input() cardStyle: string = 'm4 rounded border-divider background-card';
    @Input() tooltipLocation: TooltipPosition = 'below';

    @Input() showActions: boolean = true;
    @Input() showDetails: boolean = true;

    @Output() edit = new EventEmitter<Proposal>();
    @Output() submit = new EventEmitter<Package>();
    @Output() withdraw = new EventEmitter<Package>();
    @Output() remove = new EventEmitter<Proposal>();

    constructor(
        public packageEvents: PackageListener,
        private proposalQuery: ProposalQuery,
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
        )
        {
            await this.loadPackage();
            await this.loadPackages();            
        }
    }

    async ngOnInit(): Promise<void> {
        await this.load();
        await this.packageEvents.connect();

        this.packageEvents.onAdd.set(this.handlePackageEvent);
        this.packageEvents.onUpdate.set(this.handlePackageEvent);
        this.packageEvents.onRemove.set(this.handlePackageEvent);
        this.packageEvents.onStateChanged.set(this.handlePackageEvent);
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