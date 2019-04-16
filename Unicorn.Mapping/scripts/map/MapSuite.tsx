import { Map } from "../typings/leaflet";

declare var eventEmitter: EventEmitter;

export abstract class MapEventManager {
    constructor(private map: Map) {

    }

}