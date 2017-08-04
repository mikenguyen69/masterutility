#import pandas as pd
#import datetime as dt
#import pandas as pd
#from pandas_datareader import data as web
#import matplotlib.pyplot as plt
#from matplotlib import style
#style.use('fivethirtyeight')


###Part 1
##df = web.DataReader("F",'google',dt.datetime(2014,11,11),dt.datetime(2017,7,22))

##print(df)
##print (df.head())

##df['High'].plot()
##plt.legend()
##plt.show()

###Part 2
##web_stats = {'Day':[1,2,3,4,5,6],
##             'Visitors':[43,34,65,56,29,76],
##             'Bounce Rate':[65,67,78,65,45,52]}


##df = pd.DataFrame(web_stats)

##print (df.head())

##df.set_index('Day', inplace=True)
##print (df.head())
##print (df.tail(2))

##print(df[['Visitors', 'Bounce Rate']])
##df['Visitors'].plot()
##plt.show()

###Part 3

##print(fiddy_states[0])
#import quandl as ql
#import pandas as pd
#import pickle

## Not necessary, I just do this so I do not show my API key.
#api_key = "CYh6_q5SavmAbWZPrPvY"

##def state_list():
##    fiddy_states = pd.read_html('https://simple.wikipedia.org/wiki/List_of_U.S._states')
##    return fiddy_states[0][0][1:]
    

##def grab_initial_state_data():
##    states = state_list()

##    main_df = pd.DataFrame()

##    for abbv in states:
##        query = "FMAC/HPI_"+str(abbv)
##        df = ql.get(query, authtoken=api_key)
##        print(query)
##        if main_df.empty:
##            main_df = df
##        else:
##            main_df = main_df.append(df)
            
##    pickle_out = open('fiddy_states.pickle','wb')
##    pickle.dump(main_df, pickle_out)
##    pickle_out.close()        

    
##grab_initial_state_data()

##pickle_in = open('fiddy_states.pickle','rb')
##HPI_data = pickle.load(pickle_in)
##print(HPI_data)

##HPI_data.to_pickle('pickle_pickle')
##HPI_data2 = pd.read_pickle('pickle_pickle')
###print(HPI_data2)

##HPI_data['TX2'] = HPI_data['Value'] * 2
##print(HPI_data[['Value', 'TX2']].head())

##HPI_data.plot()
##plt.legend().remove()
##plt.show()

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
plt.style.use('ggplot')
plt.rcParams['figure.figsize'] = (10,6)

def computeCost(X, y, theta): 
    # X = (97, 2); theta = (2,1)
    # X * theta = (97, 1)
    # (X * theta)' = (1, 97)
    residual = np.dot(X, theta) - y
    m = y.shape[0]
    return 1/(2*m) * np.dot(residual.transpose(), residual)

def gradientDescent(X, y, theta, alpha, num_iters):
    m = y.shape[0]
    J_history = np.zeros_like((num_iters,1))
    for iter in range(num_iters):
        residual = np.dot(X, theta) - y
        temp = theta - (alpha/m)*np.dot(X.transpose(), residual)
        theta = temp
        J_history[iter,0] = computeCost(X, y, theta)
    return theta, J_history

data = np.loadtxt('ex1data1.txt', delimiter = ',')
X = data[:,0]
y = data[:,1]

# adding column of ones to X to account for theta_0 (the intercept)
X = np.c_[np.ones(X.shape[0]), X]
y = y.reshape(y.shape[0], 1)
theta = np.zeros_like((2,1))

# cost at the very beginning with coefficients initialized at zero 
computeCost(X, y, theta)

iterations = 1500
alpha = 0.01

optimal_theta, J_history = gradientDescent(X, y, theta, alpha, interations)

fig = plt.figure()
ax = fig.add_subplot(1,1,1)
ax.plot(X,y,'o', label='Raw Data')
plt.ylabel('Profit in $10Ks')
plt.xlabel('Population of City in 10Ks')
plt.show()

