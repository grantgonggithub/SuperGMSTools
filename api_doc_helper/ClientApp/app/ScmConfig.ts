export class ScmConfig {
    QuantumConfig: any;
    ConstKeyValue: KeyItem;
}
export class KeyItem {
    Items:  ScmConfigItem[];
}
export class ScmConfigItem {
    Key: string;
    Value: string;
}
