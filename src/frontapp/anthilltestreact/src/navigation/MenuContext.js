import React, { createContext, useState, useContext } from 'react';

const MenuContext = createContext();

export const MenuProvider = ({ children }) => {
    const [showMenu, setShowMenu] = useState(true);
    return (
        <MenuContext.Provider value={{ showMenu, setShowMenu }}>
            {children}
        </MenuContext.Provider>
    );
};

export const useMenu = () => {
    return useContext(MenuContext);
};