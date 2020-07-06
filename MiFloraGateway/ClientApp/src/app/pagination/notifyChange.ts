
export function notifyChange(target: { [key: string]: any; update: () => void; }, propertyKey: string) {
    Object.defineProperty(target, propertyKey, {
        set: function (newValue) {
            let value = newValue;
            Object.defineProperty(this, propertyKey, {
                get: () => value,
                set: (newValue) => {
                    if (value != newValue) {
                        value = newValue;
                        this.update.call(this);
                    }
                }
            });
        }
    });
}
