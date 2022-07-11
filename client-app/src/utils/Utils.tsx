import { useContext } from 'react';
import type {MainStore}  from '../stores/MainStore/MainStore';
import { MobXProviderContext } from 'mobx-react';

export function useStores(): MainStore {
    return useContext(MobXProviderContext) as MainStore;
}