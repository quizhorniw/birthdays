export interface Birthday {
    id: number;
    firstName: string;
    lastName: string;
    birthDay: string;
    photoFileName?: string | undefined;
}

export interface UpdateBirthday {
    firstName: string;
    lastName: string;
    birthDay: string;
}

export interface CreateBirthday {
    firstName: string;
    lastName: string;
    birthDay: string;
}