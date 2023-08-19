import { useState } from 'react';
import axios from 'axios';

export function useAuth() {
    const [token, setToken] = useState(localStorage.getItem('jwtToken'));

    const login = async (email, password) => {
        try {
            const response = await axios.post('http://localhost:5001/api/authentication/Login', {
                email,
                password
            });

            const token = response.data.token;
            setToken(token);
            localStorage.setItem('jwtToken', token);
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        } catch (error) {
            console.error(error.response.data.error);
        }
    };

    const logout = () => {
        setToken(null);
        localStorage.removeItem('jwtToken');
        axios.defaults.headers.common['Authorization'] = null;
    };

    return {
        token,
        login,
        logout
    };
}